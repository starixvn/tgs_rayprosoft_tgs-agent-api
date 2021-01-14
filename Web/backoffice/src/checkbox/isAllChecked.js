export const isAllChecked = (a = {}) => {
  let list = Object.values(a)
  return list.length > 0
    ? list.find((checked) => checked === false) ?? true
    : false
}
