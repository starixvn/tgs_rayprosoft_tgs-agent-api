import Big from "big.js"

export const sumColumn = (name, tableRows) =>
    sumColumnValue(name, (value) => value, tableRows)

export const sumColumnValue = (name, toValue, tableRows) => {
    let result = tableRows.reduce((total, row) => {
        let column = Reflect.get(row.values, name)

        let value = toValue(column)
        if (Number.isFinite(value)) {
            return total.plus(new Big(value))
        }
        return total
    }, new Big(0))

    return result.toNumber()
}
