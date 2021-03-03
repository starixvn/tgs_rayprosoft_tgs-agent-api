import React from "react"
import { css, Global } from "@emotion/react"

export const textColumn = ({
    textAlign = "center",
    whiteSpace = "nowrap",
    ...props
}) => ({
    headCell: {
        textAlign,
        whiteSpace
    },
    cell: {
        textAlign,
        whiteSpace
    },
    ...props
})

// export const textAlignCenterColumn = textColumn

export default textColumn
