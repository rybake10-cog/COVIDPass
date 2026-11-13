// © 2020 Partners HealthCare Technology & Innovation Department
import { createBrowserHistory } from 'history';

import { appUrlPath } from 'utils/url';

export const history = createBrowserHistory({ basename: appUrlPath });
