import React from 'react';
import PropTypes from 'prop-types';
import Header from '../components/Header';
import MemoList from '../components/MemoList';

export default class AppLayout extends React.Component {
  constructor(props) {
    super(props);
    this.onClick = this.onClick.bind(this);
  }

  onClick() {
    const { memos, createMemo } = this.props;
    createMemo(`New memo ${memos.length + 1}`);
  }

  render() {
    const { memos, removeMemo, updateMemoContent } = this.props;
    return (
      <div className="container">
        <Header />
        <button className="add-button" onClick={this.onClick}>Add</button>
        <MemoList
          memos={memos}
          removeMemo={removeMemo}
          updateMemoContent={updateMemoContent} />
      </div>
    );
  }
}

AppLayout.propTypes = {
  memos: PropTypes.array.isRequired,
  createMemo: PropTypes.func.isRequired,
  removeMemo: PropTypes.func.isRequired,
  updateMemoContent: PropTypes.func.isRequired,
};
