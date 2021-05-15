import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Box, space, styled } from "bumbag"

export const Grid = ({ areas = "", children, ...props }) => {
    return (
        <div
            css={css`
                display: grid;
                grid-template-rows: auto;
                grid-template-areas: ${areas};
            `}
            {...props}
        >
            {children}
        </div>
    )
}

//  margin: ${space(2, "major")}rem;
export const FieldValues = styled(Box)`
    display: grid;
    align-content: start;
    grid-template-columns: minmax(20%, auto) 1fr;
    gap: ${space(1, "major")}rem;
`

export const TwoColumns = styled(Box)`
    display: grid;
    grid-template-columns: ${(props) =>
        `${props?.columns?.[0] ?? "1fr"} ${props?.columns?.[1] ?? "1fr"}`};
    gap: ${space(1, "major")}rem;
`

export default Grid
