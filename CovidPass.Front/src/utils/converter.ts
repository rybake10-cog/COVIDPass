// © 2020 Partners HealthCare Technology & Innovation Department
export const getInt = (value: string, defautlValue = 0) => {
  const parsed = parseInt(value);
  return isNaN(parsed) ? defautlValue : parsed;
};
