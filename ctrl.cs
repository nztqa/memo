using System;
using System.Drawing;
using System.Windows.Forms;

namespace hogehoge
{
    /// <summary>
    /// 透かし文字
    /// </summary>
    public partial class CueDataGridViewCell : DataGridViewTextBoxCell
    {
        #region このクラスの定義

        private String _watermarkTextValue;

        #endregion このクラスの定義

        #region コンストラクタ

        public CueDataGridViewCell()
        {
            InitializeComponent();
        }

        #endregion コンストラクタ

        #region プロパティ

        public String WatermarkText
        {
            get { return _watermarkTextValue; }
            set { _watermarkTextValue = value; }
        }

        #endregion プロパティ

        #region public メソッド

        public override object Clone()
        {
            CueDataGridViewCell cell = (CueDataGridViewCell)base.Clone();
            cell.WatermarkText = this.WatermarkText;
            return cell;
        }

        #endregion public メソッド

        #region protected メソッド

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            if ((OwningColumn.Site == null || !OwningColumn.Site.DesignMode) && (RowIndex < 0 || !IsInEditMode) && !String.IsNullOrEmpty(WatermarkText) && (GetValue(rowIndex) == null || GetValue(rowIndex) == DBNull.Value))
            {
                // 透かし文字のスタイル
                cellStyle.Font = new Font(cellStyle.Font, FontStyle.Italic);
                cellStyle.ForeColor = Color.Gray;
                formattedValue = WatermarkText;
            }
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
        }

        #endregion protected メソッド
    }
}
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace hogehoge
{
    /// <summary>
    /// 透かし文字
    /// </summary>
    public partial class CueDataGridViewTextBoxColumn : DataGridViewTextBoxColumn
    {
        #region コンストラクタ

        public CueDataGridViewTextBoxColumn()
        {
            InitializeComponent();
            CellTemplate = new CueDataGridViewCell();
        }

        #endregion コンストラクタ

        #region プロパティ

        [Category("表示")]
        [DefaultValue("")]
        [Description("テキストが空の場合に表示する文字列を設定または取得します。")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public String WatermarkText
        {
            get
            {
                if (((CueDataGridViewCell)CellTemplate) == null)
                {
                    throw new InvalidOperationException("cell template required");
                }
                return ((CueDataGridViewCell)CellTemplate).WatermarkText;
            }
            set
            {
                if (this.WatermarkText != value)
                {
                    ((CueDataGridViewCell)CellTemplate).WatermarkText = value;

                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            CueDataGridViewCell cell = dataGridViewRow.Cells[this.Index] as CueDataGridViewCell;
                            if (cell != null)
                            {
                                cell.WatermarkText = value;
                            }
                        }
                    }
                }
            }
        }

        #endregion プロパティ
    }
}
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace hogehoge
{
    /// <summary>
    /// カスタム DataGridView （主に入力用）
    /// </summary>
    /// <remarks>1.EnterキーをTabキーへすり替え
    /// 2.プロパティ追加　編集中セルスタイル
    /// 3.フォーカス制御 読み取り専用セルがフォーカスを得る場合は、次のセルをさがす。
    /// 4.編集モード制御 フォーカス時値があれば編集モードへ。
    ///
    /// フォーカス、イベント発生順など良く理解しておくこと。
    /// CellEnterイベントはかなり曲者。（CellEnterイベントのCurrentCellプロパティなども）
    ///
    /// ・セルからセル（同じ行）に移動するとき
    /// 1.Cell Leave (old cell)
    /// 2.Cell Validating/ed (old cell)
    /// 3.Cell EndEdit (old cell)
    /// 4.Cell Enter (new cell)
    /// 5.Cell BeginEdit (new cell)
    ///
    /// ・ある行から別の行に移動するとき
    /// 1.Cell Leave (old cell), Row leave (old row)
    /// 2.Cell Validating/ed (old cell)
    /// 3.Cell EndEdit (old cell)
    /// 4.Row Validating/ed (old row)
    /// 5.Row Enter (new row)
    /// 6.Cell Enter (new cell)
    /// 7.Cell BeginEdit (new cell)
    /// </remarks>
    public partial class CustomDataGridView : DataGridView
    {
        #region このクラスの定義

        // 編集中のセルスタイル
        private DataGridViewCellStyle _editCellStyle;

        // DataBind完了フラグ CellEnterイベントの制御用
        private bool _isBindingComplete = false;

        #endregion このクラスの定義

        #region コンストラクタ

        public CustomDataGridView()
        {
            InitializeComponent();
            _editCellStyle = new DataGridViewCellStyle(base.DefaultCellStyle);
            _editCellStyle.BackColor = Color.Khaki;
        }

        #endregion コンストラクタ

        #region プロパティ

        /// <summary>
        /// 編集中セルのスタイルを取得または設定します。
        /// </summary>
        [Browsable(true)]
        [Category("表示")]
        [Description("編集中セルのスタイルです。")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(typeof(DataGridViewCellStyle), "DataGridViewCellStyle { BackColor=Color [Khaki] }")]
        public DataGridViewCellStyle EditCellStyle
        {
            get { return _editCellStyle; }
            set { _editCellStyle = value; }
        }

        #endregion プロパティ

        #region protected メソッド

        /// <summary>
        /// 次の編集可能なセルへ移動する。最後のセルの場合は次のコントロールへフォーカスを移動します。
        /// </summary>
        /// <returns>最後のセルが編集不可（ReadOnly）の場合は：true / 編集可能：false</returns>
        protected bool MoveToNextCell(Keys keyData)
        {
            // 最後のCellアドレス
            if (!IsMatchEditLastCellAddress())
            {
                this.EndEdit();
                SetNextTabableControl();
                return true;
            }

            bool retValue = base.ProcessTabKey(Keys.Tab);

            while (this.CurrentCell.ReadOnly)
            {
                retValue = base.ProcessTabKey(Keys.Tab);
                if (!retValue)
                    break;
            }
            return retValue;
        }

        /// <summary>
        /// 前の編集可能なセルへ移動する。先頭セルの場合は前のコントロールへフォーカスを移動します。
        /// </summary>
        /// <returns>最初のセルが編集不可（ReadOnly）の場合は：true / 編集可能：false</returns>
        protected bool MoveToPreviousCell(Keys keydata)
        {
            // 最初のCellアドレス
            if (!IsMatchEditFirstCellAddress())
            {
                this.EndEdit();
                SetPreviousTabableControl();
                return true;
            }

            bool retValue = base.ProcessTabKey(Keys.Shift | Keys.Tab);

            while (this.CurrentCell.ReadOnly)
            {
                retValue = base.ProcessTabKey(Keys.Shift | Keys.Tab);
                if (!retValue)
                    break;
            }
            return retValue;
        }

        protected override void OnCellBeginEdit(DataGridViewCellCancelEventArgs e)
        {
            this._isBindingComplete = false;

            // 編集中のセルのスタイルを変更
            this[e.ColumnIndex, e.RowIndex].Style = this._editCellStyle;
            base.OnCellBeginEdit(e);
        }

        protected override void OnCellEndEdit(DataGridViewCellEventArgs e)
        {
            // CellEnterイベント可能
            this._isBindingComplete = true;

            // 編集が終わったのでセルのスタイルを変更
            this[e.ColumnIndex, e.RowIndex].Style = null;
            base.OnCellEndEdit(e);
        }

        /// <summary>
        /// 現在のセルが変更されたとき、またはこのコントロールが入力フォーカスを受け取ったときに発生します。
        /// </summary>
        /// <remarks>コントロールに入力フォーカスがなく、クリックされたセルが以前に現在のセルではなかった場合、このイベントが 1 回のクリックに対して 2 回発生することがあります。</remarks>
        protected override void OnCellEnter(DataGridViewCellEventArgs e)
        {
            // HACK: どうするかな
            // DataSourceBinding未完了、Cell編集中なら抜ける。
            if (!this._isBindingComplete)
                return;

            // Cell値がnullなら抜ける。
            if (this[e.ColumnIndex, e.RowIndex].Value == null)
                return;

            // セルの値があるなら編集モードへ
            if (!string.IsNullOrWhiteSpace(this[e.ColumnIndex, e.RowIndex].Value.ToString()))
            {
                this.BeginEdit(true);
            }

            base.OnCellEnter(e);
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            // 行ヘッダーに行番号を表示する
            // 列ヘッダーかどうか調べる
            // e.AdvancedBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;
            if (e.ColumnIndex < 0 && e.RowIndex >= 0)
            {
                //セルを描画する
                e.Paint(e.ClipBounds, DataGridViewPaintParts.All);

                // 行番号を描画する範囲を決定する
                // e.AdvancedBorderStyleやe.CellStyle.Paddingは無視しています
                Rectangle indexRect = e.CellBounds;
                indexRect.Inflate(-2, -2);

                // 行番号を描画する
                if (e.ErrorText == "")
                {
                    TextRenderer.DrawText(e.Graphics,
                        (e.RowIndex + 1).ToString(),
                        e.CellStyle.Font,
                        indexRect,
                        e.CellStyle.ForeColor,
                        TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
                }

                // 描画が完了したことを知らせる
                e.Handled = true;
                return;
            }

            //
            // 選択したセルを太線で表示(Excelの用に
            // 標準は点線ですが、Win7では視認できません。XPでは視認できます。
            //
            //http://blog.syo-ko.com/?eid=1070
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0 && (e.PaintParts & DataGridViewPaintParts.Background) == DataGridViewPaintParts.Background)
            {
                e.Graphics.FillRectangle(new SolidBrush(e.CellStyle.BackColor), e.CellBounds);

                if ((e.PaintParts & DataGridViewPaintParts.SelectionBackground) == DataGridViewPaintParts.SelectionBackground && (e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected)
                {
                    e.Graphics.DrawRectangle(new Pen(Color.Black, 2), e.CellBounds.X + 1, e.CellBounds.Y + 1, e.CellBounds.Width - 3, e.CellBounds.Height - 3);
                }

                DataGridViewPaintParts pParts = e.PaintParts & ~DataGridViewPaintParts.Background;

                e.Paint(e.ClipBounds, pParts);

                e.Handled = true;
            }
            base.OnCellPainting(e);
        }

        protected override void OnCellValidated(DataGridViewCellEventArgs e)
        {
            // エラークリア
            this.Rows[e.RowIndex].ErrorText = string.Empty;
            base.OnCellValidated(e);
        }

        protected override void OnColumnStateChanged(DataGridViewColumnStateChangedEventArgs e)
        {
            if (e.StateChanged == DataGridViewElementStates.Visible)
            {
                if (!e.Column.Visible && e.Column.Displayed)
                {
                    if (this.CurrentCell == null)
                    {
                        // HACK: 列を非表示にしたら、同じCellAddressへCurrentcCellを設定したい
                        //　OnColumnStateChangedではRow位置が取得できない
                    }
                }
            }
            base.OnColumnStateChanged(e);
        }
        protected override void OnDataBindingComplete(DataGridViewBindingCompleteEventArgs e)
        {
            // CellEnterイベント可能
            this._isBindingComplete = true;
            base.OnDataBindingComplete(e);
        }

        protected override void OnDataMemberChanged(EventArgs e)
        {
            this._isBindingComplete = false;
            base.OnDataMemberChanged(e);
        }

        protected override void OnDataSourceChanged(EventArgs e)
        {
            this._isBindingComplete = false;
            base.OnDataSourceChanged(e);
        }

        /// <summary>
        /// DataGridView での移動に使用されるキーを処理します。
        /// </summary>
        protected override bool ProcessDataGridViewKey(KeyEventArgs e)
        {
            KeyEventArgs eTemp = e; // キーイベント

            // TabとEnterキーならReadOnlyCellを飛ばす
            // 矢印キーや、Home,End,PageUp,PageDownなども制御したい場合は追加する事。
            switch (e.KeyData)
            {
                case Keys.Tab:
                    if (MoveToNextCell(Keys.Tab))
                    {
                        return true;
                    }
                    break;

                case Keys.Enter:
                    if (MoveToNextCell(Keys.Enter))
                    {
                        return true;
                    }

                    //eTemp = new KeyEventArgs(Keys.Tab);
                    break;

                case Keys.Shift | Keys.Tab:
                    if (MoveToPreviousCell(Keys.Shift | Keys.Tab))
                    {
                        return true;
                    }
                    break;

                case Keys.Shift | Keys.Enter:
                    if (MoveToPreviousCell(Keys.Shift | Keys.Enter))
                    {
                        return true;
                    }

                    break;

                default:
                    break;
            }
            return base.ProcessDataGridViewKey(eTemp);
        }

        /// <summary>
        /// 制御するために使用されるキーをを処理します。（Tab キー、Esc キー、Enter キー、方向キーなど）
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            // keyData から KeyCode を得るには　Keys.KeyCode との And を取る。
            // 修飾キーも同じ様に Keys.Modifiers で And を取らないと、別のキーとの組み合わせの際に拾うことはできない。
            // Keys key = (keyData & Keys.KeyCode);

            // TabとEnterキーならReadOnlyCellを飛ばす
            // 矢印キーや、Home,End,PageUp,PageDownなども制御したい場合は追加する事。
            switch (keyData)
            {
                case Keys.Tab:
                    if (MoveToNextCell(Keys.Tab))
                    {
                        return true;
                    }
                    break;

                case Keys.Enter:
                    if (MoveToNextCell(Keys.Enter))
                    {
                        return true;
                    }

                    //keyData = Keys.Tab;
                    break;

                case Keys.Shift | Keys.Tab:
                    if (MoveToPreviousCell(Keys.Shift | Keys.Tab))
                    {
                        return true;
                    }
                    break;

                case Keys.Shift | Keys.Enter:
                    if (MoveToPreviousCell(Keys.Shift | Keys.Enter))
                    {
                        return true;
                    }
                    break;

                default:
                    break;
            }
            return base.ProcessDialogKey(keyData);
        }

        #endregion protected メソッド

        #region private メソッド

        /// <summary>
        /// 編集可能なCellAddressが表示上の最初のCellAddressと一致しているか。
        /// </summary>
        /// <returns>最初のセルが編集不可（ReadOnly）の場合は：false / 編集可能：true</returns>
        private bool IsMatchEditFirstCellAddress()
        {
            bool retValue = false;
            int rowAddress = 0;

            // 編集可能なCell　ReadOnly = false, Visible = true
            var editFirstColumn = this.Columns.GetFirstColumn(DataGridViewElementStates.Visible, DataGridViewElementStates.ReadOnly);

            if (editFirstColumn == null)
                return false;

            // 編集可能な最初のセルが表示上の最初のセルと一致
            if ((this.CurrentCellAddress.X == editFirstColumn.Index) && (this.CurrentCellAddress.Y == rowAddress))
            {
                retValue = false;
            }
            else
            {
                retValue = true;
            }

            if (Rows.Count == 0)
                retValue = false;

            return retValue;
        }

        /// <summary>
        /// 編集可能なCellAddressが表示上の最後のCellAddressと一致しているか。
        /// </summary>
        /// <returns>最後のセルが編集不可（ReadOnly）の場合は：false / 編集可能：true</returns>
        private bool IsMatchEditLastCellAddress()
        {
            bool retValue = false;
            int rowAddress = this.Rows.Count - 1;

            // 編集可能なCell　ReadOnly = false, Visible = true
            var editLastColumn = this.Columns.GetLastColumn(DataGridViewElementStates.Visible, DataGridViewElementStates.ReadOnly);

            if (editLastColumn == null)
                return false;

            // 編集可能な最後のセルが表示上の最後のセルと一致 this.CurrentCell.ColumnIndex
            if ((this.CurrentCellAddress.X == editLastColumn.Index) && (this.CurrentCellAddress.Y == rowAddress))
            {
                retValue = false;
            }
            else
            {
                retValue = true;
            }

            if (Rows.Count == 0)
                retValue = false;

            return retValue;
        }

        /// <summary>
        /// 次のコントロールへフォーカスを移動します。
        /// </summary>
        private void SetNextTabableControl()
        {
            this.Focus();
            this.FindForm().SelectNextControl(this, true, true, true, true);
        }

        /// <summary>
        /// 前のコントロールへフォーカスを移動します。
        /// </summary>
        private void SetPreviousTabableControl()
        {
            this.Focus();
            this.FindForm().SelectNextControl(this, false, true, true, true);
        }

        #endregion private メソッド
    }
}
