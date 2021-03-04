import addDays from "date-fns/addDays"
import startOfDay from "date-fns/startOfDay"

export const startOfNextDay = (value) => {
  let nextDay = addDays(value, 1)
  return startOfDay(nextDay)
}
