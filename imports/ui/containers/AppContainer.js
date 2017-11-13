import { withTracker } from 'meteor/react-meteor-data';
import AppLayout from '../layouts/AppLayout.js';
import { Memos } from '../../api/memos/memos.js';
import { insert, update, remove } from '../../api/memos/methods.js'

const createMemo = content => {
  insert.call({content});
};

const removeMemo = memoId => {
  remove.call({memoId});
};

const updateMemoContent = (memoId, content) => {
  update.call({memoId, content})
};

export default withTracker(() => {
  const memosHandle = Meteor.subscribe('memos.all');
  return {
    createMemo,
    removeMemo,
    updateMemoContent,
    memos: Memos.find().fetch(),
  };
})(AppLayout);
