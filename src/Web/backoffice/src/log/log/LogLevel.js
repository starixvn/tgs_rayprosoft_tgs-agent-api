import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import Select from "../../select/Select"

export const All = "All"

export const Verbose = "Verbose"

export const Debug = "Debug"

export const Info = "Info"

export const Warning = "Warning"

export const Error = "Error"

export const Fatal = "Fatal"

export default {
    All,
    Verbose,
    Debug,
    Info,
    Warning,
    Error,
    Fatal
}

export const LogLevelSelect = ({ ...props }) => {
    let options = [
        {
            label: All,
            value: All
        },
        {
            label: Verbose,
            value: Verbose
        },
        {
            label: Debug,
            value: Debug
        },
        {
            label: Info,
            value: Info
        },
        {
            label: Warning,
            value: Warning
        },
        {
            label: Error,
            value: Error
        },
        {
            label: Fatal,
            value: Fatal
        }
    ]

    return <Select options={options} {...props} />
}
