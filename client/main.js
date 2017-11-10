import { Meteor } from 'meteor/meteor';
import React from 'react';
import { render } from 'react-dom';

Meteor.startup(() => {
  render(
    <h1>Hello, Meteor React World!</h1>,
    document.getElementById('render-root')
  );
});
