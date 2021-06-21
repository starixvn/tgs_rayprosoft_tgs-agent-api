import React from "react"
import { css, Global } from "@emotion/react"

const DefaultHeadCellStyle = {}
const DefaultCellStyle = {}
export const textColumn = ({
    width,
    maxWidth,
    textAlign = "center",
    whiteSpace = "normal",
    overflow = "normal",
    overflowWrap = "normal",
    textOverflow,
    wordBreak = "normal",
    ...props
}) => ({
    headCell: {
        width,
        maxWidth,
        textAlign,
        whiteSpace,
        wordBreak
    },
    cell: {
        width,
        maxWidth,
        textAlign,
        whiteSpace,
        wordBreak,
        overflow,
        textOverflow
    },
    ...props
})

export const noWrapTextColumn = ({ whiteSpace = "nowrap", ...props }) =>
    textColumn({ whiteSpace, ...props })

export const wrapTextColumn = ({
    wordBreak = "break-word",
    overflowWrap = "break-word",
    whiteSpace = "normal",
    ...props
}) => textColumn({ wordBreak, overflowWrap, whiteSpace, ...props })

export const ellipsisTextColumn = ({
    textOverflow = "ellipsis",
    overflow = "hidden",
    whiteSpace = "nowrap",
    width = "min-content",
    ...props
}) => textColumn({ textOverflow, overflow, whiteSpace, ...props })

// export const textAlignCenterColumn = textColumn

export default textColumn
