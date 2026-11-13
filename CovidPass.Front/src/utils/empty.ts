// © 2020 Partners HealthCare Technology & Innovation Department
export const isNotEmpty = (parameter: string | undefined | null) => !isEmpty(parameter);
export const isEmpty = (parameter: string | undefined | null) => parameter == null || parameter === '';
