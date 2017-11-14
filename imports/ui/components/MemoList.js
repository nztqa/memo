import React from 'react';
import PropTypes from 'prop-types';
import MemoItem from './MemoItem';
import Masonry from 'react-masonry-component';

const masonryOptions = {
  transitionDuration: 200,
};

export default class MemoList extends React.Component {
  render() {
    const { memos, removeMemo, updateMemoContent } = this.props;
    return (
      <div className="memo-list">
        <Masonry options={masonryOptions}>
          {memos.map(memo => (
            <MemoItem
              key={memo._id}
              memo={memo}
              removeMemo={removeMemo}
              updateMemoContent={updateMemoContent} />
          ))}
        </Masonry>
      </div>
    );
  }
}

MemoList.propTypes = {
  memos: PropTypes.array.isRequired,
  removeMemo: PropTypes.func.isRequired,
  updateMemoContent: PropTypes.func.isRequired,
};
