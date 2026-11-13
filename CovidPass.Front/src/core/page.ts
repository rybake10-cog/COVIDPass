// © 2020 Partners HealthCare Technology & Innovation Department
import { ParsedQuery } from 'query-string';

export interface Page<T> {
  items: T[];
  totalPages: number;
  totalItems: number;
  currentPage: number;
  pageSize: number;
}

export type PageParams = ParsedQuery<string | number | boolean>;
