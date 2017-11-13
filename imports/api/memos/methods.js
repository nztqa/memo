import { Meteor } from 'meteor/meteor';
import { ValidatedMethod } from 'meteor/mdg:validated-method';
import SimpleSchema  from 'simpl-schema';
import { Memos } from './memos';

export const insert = new ValidatedMethod({
  name: 'memos.insert',
  validate: new SimpleSchema({
    content: {
      type: String,
    },
  }).validator(),
  run({ content }) {
    this.unblock();
    Memos.insert({content});
  },
});

export const update = new ValidatedMethod({
  name: 'memos.update',
  validate: new SimpleSchema({
    memoId: {
      type: String,
    },
    content: {
      type: String,
    },
  }).validator(),
  run({ memoId, content }) {
    this.unblock();
    Memos.update({_id: memoId}, {$set: {content}});
  },
});

export const remove = new ValidatedMethod({
  name: 'memos.remove',
  validate: new SimpleSchema({
    memoId: {
      type: String,
    },
  }).validator(),
  run({ memoId }) {
    this.unblock();
    Memos.remove(memoId);
  },
});
