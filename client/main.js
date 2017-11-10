import { Meteor } from 'meteor/meteor';
import React from 'react';
import { render } from 'react-dom';
import AppLayout from '../imports/ui/layouts/AppLayout.js';

Meteor.startup(() => {
  render(
    <AppLayout />,
    document.getElementById('render-root')
  );
});
