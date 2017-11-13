import { Mongo } from 'meteor/mongo';
import SimpleSchema from 'simpl-schema';

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

// for debug
if (Meteor.isDevelopment) {
  global.collections = global.collections || {};
  global.collections.Memos = Memos;
}
