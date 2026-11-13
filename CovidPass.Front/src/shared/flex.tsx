// © 2020 Partners HealthCare Technology & Innovation Department
import './flex.scss';

import React from 'react';
import classNames from 'classnames';

export interface FlexProps extends React.HTMLAttributes<any> {
  justifyContent?: 'flex-start' | 'flex-end' | 'center' | 'space-between' | 'space-around' | 'space-evenly';
  alignItems?: 'stretch' | 'flex-start' | 'flex-end' | 'center' | 'baseline';
  wrap?: 'nowrap' | 'wrap' | 'wrap-reverse';
  direction?: 'row' | 'row-reverse' | 'column' | 'column-reverse';
}

export const Flex: React.FC<FlexProps> = ({
  className,
  justifyContent,
  alignItems,
  wrap,
  direction,
  children,
  style,
  ...other
}) => {
  return (
    <div
      className={classNames('flex', className)}
      style={{ ...style, justifyContent, alignItems, flexWrap: wrap, flexDirection: direction }}
      {...other}>
      {children}
    </div>
  );
};
