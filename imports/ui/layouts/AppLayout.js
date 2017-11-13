import React from 'react';
import PropTypes from 'prop-types';
import Header from '../components/Header';
import MemoList from '../components/MemoList';

export default class AppLayout extends React.Component {
  render() {
    const { memos } = this.props;
    return (
      <div className="container">
        <Header />
        <MemoList memos={memos} />
      </div>
    );
  }
}

AppLayout.propTypes = {
  memos: PropTypes.array.isRequired,
};
