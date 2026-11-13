// © 2020 Partners HealthCare Technology & Innovation Department
import React from 'react';
import classNames from 'classnames';

import { Flex } from 'shared';

import './field.scss';

interface Props extends React.HTMLAttributes<any> {
  view?: 'row' | 'responsive-row' | 'column';
  text: React.ReactNode;
  justifyContent?: 'center' | 'space-between';
}

export const Field: React.FC<Props> = ({
  view = 'column',
  text,
  justifyContent = 'space-between',
  className,
  children,
  ...other
}) => (
  <Flex
    direction={view != 'responsive-row' ? view : undefined}
    justifyContent={justifyContent}
    alignItems={view === 'row' ? 'center' : undefined}
    className={classNames('form-field', { 'responsive-row': view === 'responsive-row' }, className)}
    {...other}>
    <div className="form-label">{text}</div>
    <Flex className="form-controls-container" alignItems="center">
      {children}
    </Flex>
  </Flex>
);
