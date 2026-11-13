// © 2020 Partners HealthCare Technology & Innovation Department
import 'core-js';

import React from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import { Router } from 'react-router-dom';

import { store } from 'core/store';
import { history } from 'core/history';
import { App } from 'app/app';

import './styles.scss';

import 'core/translation';

const root = (
  <Provider store={store}>
    <Router history={history}>
      <App />
    </Router>
  </Provider>
);

ReactDOM.render(root, document.getElementById('root'));
