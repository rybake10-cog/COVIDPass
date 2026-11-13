// © 2020 Partners HealthCare Technology & Innovation Department
import { ValidationError } from 'yup';

import { DateTime } from 'utils/dateTime';

export function parseError(error: any): string | string[] {
  if (error == null)
    return `Unable to connect to CovidPass servers due to network or internet 
      connectivity issues at ${DateTime.format(new Date(), 'datetime')}.`;
  if (error instanceof ValidationError) return error.errors;
  console.error(error, typeof error);
  if (error.status === 400) {
    return error.response && error.response.isError ? error.response.message : 'Bad request';
  }
  if (error.status === 500) {
    return `Error encountered on the server at ${DateTime.format(new Date(), 'datetime')}.`;
  }
  if (error.message?.includes('ajax'))
    return `Unable to connect to CovidPass servers due to network or internet 
      connectivity issues at ${DateTime.format(new Date(), 'datetime')}.`;
  return 'Unknown error: ' + DateTime.format(new Date(), 'datetime');
}
