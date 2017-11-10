import React from 'react';
import PropTypes from 'prop-types';

export default class MemoItem extends React.Component {
  render() {
    const { memo } = this.props;
    return (
      <div className="memo-item">
        <textarea className="textarea" defaultValue={memo.content} />
      </div>
    );
  }
}

MemoItem.propTypes = {
  memo: PropTypes.object.isRequired,
};
