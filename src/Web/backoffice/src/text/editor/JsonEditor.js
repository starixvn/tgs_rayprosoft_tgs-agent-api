import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import {
    Box,
    Button,
    Text,
    Set,
    Icon,
    useModalState,
    Dialog,
    Card,
    Flex,
    Heading,
    FieldStack
} from "bumbag"
import MonacoEditor, {
    DiffEditor,
    useMonaco,
    loader
} from "@monaco-editor/react"
import { useForm, useFormContext, Controller } from "react-hook-form"
//TODO: obj -> pretty compact?
//import stringifyPrettyCompact from "json-stringify-pretty-compact"
import { stringifyJSON } from "../json"
import clone from "clone-deep"

const DefaultJsonOptions = {
    colorDecorators: true,
    contextmenu: false,
    minimap: { enabled: true },
    hideCursorInOverviewRuler: false
}

export const JsonEditor = ({ options, disabled = false, ...props }) => {
    if (!options) {
        options = clone(DefaultJsonOptions)
    }

    if (disabled) {
        options.readOnly = true
    }

    return <Body options={options} {...props} />
}

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
    let [errorMarkers, setErrorMarkers] = useState([])
    if (name && formMethods) {
        return (
            <Controller
                control={formMethods.control}
                name={name}
                defaultValue={stringifyJSON(value)}
                rules={{
                    required,
                    validate: () => {
                        return errorMarkers.length ? false : true
                    }
                }}
                render={({
                    field: { onBlur, value, name, ref },
                    fieldState: { invalid, isTouched, isDirty, error }
                }) => {
                    return (
                        <Set orientation="vertical">
                            <Editor
                                value={value}
                                {...props}
                                onValidate={setErrorMarkers}
                                onChange={(value, event) => {
                                    formMethods.setValue(name, value)
                                    onChange?.(value)
                                }}
                            />
                        </Set>
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
            theme="light"
            defaultLanguage="json"
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
