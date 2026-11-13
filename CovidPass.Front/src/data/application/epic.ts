// © 2020 Partners HealthCare Technology & Innovation Department
import { map } from 'rxjs/operators';
import { combineEpics } from 'redux-observable';

import { createEpic } from 'core/epic';
import { DateTime } from 'utils/dateTime';

import { submitAsync, setApplication } from './actions';
import { submit } from './api';

const submitEpic = createEpic(submitAsync, data =>
  submit(data).pipe(
    map(response => {
      response.date = DateTime.epicInit(response.date);
      response.created = DateTime.epicInit(response.created);
      return setApplication(response);
    })
  )
);

export const applicationEpic = combineEpics(submitEpic);
