import React from "react"
import { css, Global } from "@emotion/react"
import { FormattedNumber } from "../../i18n"
import { sumColumn } from "../sumColumn"

const DefaultNumberFormat = "0,0"

export const numberColumn = ({
    Cell,
    format = DefaultNumberFormat,
    sumColumn: sumColumnName = "",
    ...props
}) => {
    let basic = {
        Cell: ({ value }) => <FormattedNumber value={value} format={format} />,
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
export const numberDecimalColumn = (props) =>
    numberColumn({ format: DefaultNumberDecimal, ...props })

export default numberColumn
