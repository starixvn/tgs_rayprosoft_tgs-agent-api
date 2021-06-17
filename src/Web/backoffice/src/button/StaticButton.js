import React, { useState, useEffect, useMemo, useRef, useContext } from "react"
import { css, Global } from "@emotion/react"
import { Button } from "bumbag"

export const StaticButton = ({
    isStatic = true,
    tabIndex = "-1",
    children,
    ...props
}) => (
    <Button isStatic={isStatic} tabIndex={tabIndex} {...props}>
        {children}
    </Button>
)

export default StaticButton
