import { Mongo } from 'meteor/mongo';
import SimpleSchema from 'simpl-schema';
import { Factory } from 'meteor/dburles:factory';

// class MemosCollection extends Mongo.Collection {
//   insert(doc, callback) {
//     doc.createdAt = doc.createdAt || new Date();
//     const result = super.insert(doc, callback);
//     return result;
//   }
// }

export const Memos = new Mongo.Collection('Memos');

const Schema = {};
Schema.Memos = new SimpleSchema({
  content: {
    type: String,
  },
  createdAt: {
    type: Date,
    autoValue() { // eslint-disable-line consistent-return
      if (this.isInsert) {
        return new Date();
      }
    },
  },
});

Memos.attachSchema(Schema.Memos);

Memos.deny({
  insert() { return true; },
  update() { return true; },
  remove() { return true; },
});

Memos.allow({
  insert() { return false; },
  update() { return false; },
  remove() { return false; },
});

// for debug
if (Meteor.isDevelopment) {
  global.collections = global.collections || {};
  global.collections.Memos = Memos;
}

Factory.define('memo', Memos, {
  content: () => 'Factory Content',
});
