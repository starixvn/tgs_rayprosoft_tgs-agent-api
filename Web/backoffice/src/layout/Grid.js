import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import Async, { useAsync, IfRejected, IfFulfilled } from "react-async"
import { useLocation, useRouteMatch } from "react-router-dom"
import { Formik, Field, Form, useFormikContext } from "formik"
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
export const FieldValuePair = styled(Box)`
  display: grid;
  grid-template-columns: minmax(20%, auto) 1fr;
  gap: ${space(1, "major")}rem;
`

export default Grid
