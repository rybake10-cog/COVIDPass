// © 2020 Partners HealthCare Technology & Innovation Department
import React, { HTMLProps } from 'react';
import classNames from 'classnames';

interface Props extends HTMLProps<HTMLDivElement> {
  className?: string;
}

export const componentFactory: (classes?: string, tag?: React.ElementType) => React.FC<Props> = (
  classes,
  Tag = 'div'
) => ({ className, children, ...other }) => (
  <Tag className={classNames(classes, className)} {...other}>
    {children}
  </Tag>
);
