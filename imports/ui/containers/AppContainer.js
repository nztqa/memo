import { withTracker } from 'meteor/react-meteor-data';
import AppLayout from '../layouts/AppLayout.js';
import { Memos } from '../../api/memos/memos.js';

export default withTracker(() => {
  const memosHandle = Meteor.subscribe('memos.all');
  return {
    memos: Memos.find().fetch(),
  };
})(AppLayout);
