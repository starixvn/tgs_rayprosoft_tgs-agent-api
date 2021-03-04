import Big from "big.js"

export const sumColumn = (name, tableRows) => {
  let result = tableRows.reduce((total, row) => {
    let value = Reflect.get(row.values, name)
    if (Number.isFinite(value)) {
      return total.plus(new Big(value))
    }
    return total
  }, new Big(0))

  return result.toNumber()
}
