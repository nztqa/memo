import { Meteor } from 'meteor/meteor';
import React from 'react';
import { render } from 'react-dom';
import AppLayout from '../imports/ui/layouts/AppLayout.js';

// sample data
const memos = [
  {_id: 'memo1', content: 'This is sample data 1'},
  {_id: 'memo2', content: 'This is sample data 2'},
  {_id: 'memo3', content: 'This is sample data 3'},
  {_id: 'memo4', content: 'This is sample data 4'},
  {_id: 'memo5', content: 'This is sample data 5'},
];

Meteor.startup(() => {
  render(
    <AppLayout memos={memos} />,
    document.getElementById('render-root')
  );
});
