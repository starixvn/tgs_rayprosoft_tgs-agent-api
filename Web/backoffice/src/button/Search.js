import React, { useState, useEffect, useMemo, useRef, useContext } from "react"
import { css, Global } from "@emotion/core"
import { Button as _Button } from "bumbag"
import { useTrans, Trans } from "../i18n"

const Button = ({ type = "submit", children, ...props }) => (
  <_Button type={type} {...props}>
    {children ?? <Trans zh="搜索" en="Search" />}
  </_Button>
)

export default Button
