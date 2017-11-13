import React from 'react';
import PropTypes from 'prop-types';

export default class MemoItem extends React.Component {
  constructor(props) {
    super(props);
    const { memo } = this.props;
    this.state = {
      textAreaValue: memo.content
    };
    this.onClick = this.onClick.bind(this);
    this.onChange = this.onChange.bind(this);
  }

  onClick(event) {
    event.preventDefault();
    const { memo, removeMemo } = this.props;
    removeMemo(memo._id);
  }

  onChange(event) {
    const { memo, updateMemoContent } = this.props;
    const content = event.target.value;
    this.setState({
      textAreaValue: content
    });
    updateMemoContent(memo._id, content);
  }

  componentWillReceiveProps(nextProps) {
    if (this.state.textAreaValue !== nextProps.memo.content) {
      this.setState({
        textAreaValue: nextProps.memo.content
      });
    }
  }

  render() {
    const { memo } = this.props;
    const { textAreaValue } = this.state;
    return (
      <div className="memo-item">
        <a href="#" onClick={this.onClick} className="remove-button">
          &times;
        </a>
        <textarea
          className="textarea"
          value={textAreaValue}
          onChange={this.onChange} />
      </div>
    );
  }
}

MemoItem.propTypes = {
  memo: PropTypes.object.isRequired,
  removeMemo: PropTypes.func.isRequired,
  updateMemoContent: PropTypes.func.isRequired,
};
