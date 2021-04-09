import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import {
    Box,
    Button,
    Set,
    Icon,
    useModalState,
    Dialog,
    Card,
    Flex,
    Heading,
    FieldStack
} from "bumbag"
import MonacoEditor, { DiffEditor } from "@monaco-editor/react"
import { useForm, useFormContext, Controller } from "react-hook-form"
//TODO: obj -> pretty compact?
//import stringifyPrettyCompact from "json-stringify-pretty-compact"
import { stringifyJSON } from "../json"
import clone from "clone-deep"

const DefaultJsonOptions = {
    contextmenu: false,
    minimap: { enabled: false },
    contextmenu: false,
    hideCursorInOverviewRuler: true
}

export const JsonEditor = ({
    options = DefaultJsonOptions,
    disabled = false,
    ...props
}) => {
    if (!options) {
        options = clone(DefaultJsonOptions)
    }

    if (disabled) {
        options.readOnly = true
    }

    return <Body options={options} {...props} />
}

//TODO: validate JSON
const Body = ({
    name,
    value = "",
    required,
    /*
    output = "compact",
    */
    onChange,
    ...props
}) => {
    let formMethods = useFormContext()

    if (name && formMethods) {
        return (
            <Controller
                control={formMethods.control}
                name={name}
                defaultValue={stringifyJSON(value)}
                rules={{ required }}
                render={({
                    field: { onBlur, value, name, ref },
                    fieldState: { invalid, isTouched, isDirty, error }
                }) => {
                    return (
                        <Editor
                            value={value}
                            {...props}
                            onChange={(value, event) => {
                                formMethods.setValue(name, value)
                                onChange?.(value)
                            }}
                        />
                    )
                }}
            />
        )
    }

    return (
        <Editor value={stringifyJSON(value)} {...props} onChange={onChange} />
    )
}

const Editor = ({ ...props }) => {
    return (
        <MonacoEditor
            defaultLanguage="JSON"
            css={css`
                border: 1px solid var(--bb-palette-white900, #000000);
                border-radius: 6px;
            `}
            {...props}
        />
    )
}

//TODO
export const JsonDiffEditor = () => {}
