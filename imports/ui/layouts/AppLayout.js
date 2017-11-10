import React from 'react';
import PropTypes from 'prop-types';
import Header from '../components/Header';
import MemoList from '../components/MemoList';

// sample data
const memos = [
  {_id: 'memo1', content: 'This is sample data 1'},
  {_id: 'memo2', content: 'This is sample data 2'},
  {_id: 'memo3', content: 'This is sample data 3'},
  {_id: 'memo4', content: 'This is sample data 4'},
  {_id: 'memo5', content: 'This is sample data 5'},
];

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
