import subWeeks from "date-fns/fp/subWeeks"

export const pastWeek = () => {
  let end = new Date()
  let start = subWeeks(1, end)

  return {
    start,
    end
  }
}
