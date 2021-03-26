import React from "react"
import { css, Global } from "@emotion/react"
import { FormattedNumber } from "../../i18n"
import { sumColumn } from "../sumColumn"

const DefaultNumberFormat = "0,0"

export const numberColumn = ({
    Cell,
    format = DefaultNumberFormat,
    postfix = "",
    sumColumn: sumColumnName = "",
    ...props
}) => {
    let basic = {
        Cell: ({ value }) => (
            <FormattedNumber value={value} format={format} postfix={postfix} />
        ),
        headCell: {
            textAlign: "center",
            whiteSpace: "nowrap"
        },
        cell: {
            textAlign: "center",
            whiteSpace: "nowrap"
        }
    }

    if (sumColumnName) {
        basic = {
            ...basic,
            Footer: (table) => (
                <FormattedNumber
                    value={sumColumn(sumColumnName, table.rows)}
                    format={format}
                />
            ),
            footerCellProps: {
                textAlign: "center"
            }
        }
    }

    return {
        ...basic,
        ...props
    }
}

const DefaultNumberDecimal = "0,0.00"
export const numberDecimalColumn = ({
    format = DefaultNumberDecimal,
    ...props
}) => numberColumn({ format, ...props })

export const coloredNumberDecimalColumn = ({
    value,
    format = DefaultNumberDecimal,
    postfix,
    ...props
}) =>
    numberDecimalColumn({
        format,
        Cell: ({ value }) => {
            let isPositive = value >= 0
            return (
                <Text color={isPositive ? "success" : "danger"}>
                    <FormattedNumber
                        value={value}
                        format={format}
                        postfix={postfix}
                    />
                </Text>
            )
        },
        ...props
    })

export const numberDecimalPercentageColumn = ({
    format = DefaultNumberDecimal,
    postfix = "%",
    ...props
}) => numberColumn({ format, postfix, ...props })

export default numberColumn
