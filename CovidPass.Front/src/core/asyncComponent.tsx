// © 2020 Partners HealthCare Technology & Innovation Department
import React, { FunctionComponent, Suspense } from 'react';

import { Flex, Icon } from 'shared';

const fallback = (
  <Flex justifyContent="center" alignItems="center">
    <Icon name="spinner" spin></Icon> Loading...
  </Flex>
);

export const asyncComponent = (get: () => Promise<FunctionComponent<any>>): React.FC => {
  const Comp = React.lazy(() => get().then(x => ({ default: x })));
  return () => (
    <Suspense fallback={fallback}>
      <Comp></Comp>
    </Suspense>
  );
};
