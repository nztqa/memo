import { Mongo } from 'meteor/mongo';

export const Memos = new Mongo.Collection('Memos');

// for debug
if (Meteor.isDevelopment) {
  global.collections = global.collections || {};
  global.collections.Memos = Memos;
}
