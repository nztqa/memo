import { Meteor } from 'meteor/meteor';
import { assert } from 'meteor/practicalmeteor:chai';
import { resetDatabase } from 'meteor/xolvio:cleaner';
import { Factory } from 'meteor/dburles:factory';
import { Memos } from './memos.js';
import { insert, update, remove } from './methods.js'

describe('Memos methods', () => {
  beforeEach(() => {
    if (Meteor.isServer) {
      resetDatabase();
    }
  });

  it('inserts a memo into the Memos collection', () => {
    insert.call({content: 'test memo'});
    const getMemo = Memos.findOne({content: 'test memo'});
    assert.equal(getMemo.content, 'test memo');
  });

  it('updates a memo from the Memos collection', () => {
    const { _id } = Factory.create('memo');
    update.call({ memoId: _id, content: 'test update memo' });
    const getMemo = Memos.findOne(_id);
    assert.equal(getMemo.content, 'test update memo');
  });

  it('removes a memo from the Memos collection', () => {
    const { _id } = Factory.create('memo');
    remove.call({ memoId: _id });
    const getMemo = Memos.findOne(_id);
    assert.equal(getMemo, undefined);
  });
});
