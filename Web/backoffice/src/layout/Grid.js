import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import Async, { useAsync, IfRejected, IfFulfilled } from "react-async"
import { useLocation, useRouteMatch } from "react-router-dom"
import { Formik, Field, Form, useFormikContext } from "formik"

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

export default Grid
