import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext, Controller, useWatch } from "react-hook-form"
import { Locales, useLocale } from "../../i18n"
import { Set, Box } from "bumbag"
import {
    LocalizationDialogDisclosure,
    LocalizationDialogMode
} from "../../dialog/LocalizationDialog"

const DefaultPlugin =
    "preview paste importcss searchreplace autolink code visualchars fullscreen image link media codesample table charmap hr pagebreak nonbreaking anchor toc insertdatetime advlist lists imagetools textpattern noneditable charmap emoticons"
const DefaultMenubar = "file edit insert view format table upload"
const DefaultMenu = {
    upload: { title: "Upload", items: "image", icon: "upload" }
}
const DefaultToolbar = [
    "searchreplace | bold italic underline strikethrough | alignleft aligncenter alignright alignjustify | outdent indent | undo redo | removeformat superscript subscript code codesample",
    "hr numlist bullist | link insertfile image preview anchor pagebreak insertdatetime media table | emoticons forecolor backcolor fullscreen "
]

//options refer to https://www.tiny.cloud/docs/configure/
const toOptions = ({
    selector,
    width,
    height,
    readOnly,
    setup,
    language,
    plugins = DefaultPlugin,
    menubar = DefaultMenubar,
    menu = DefaultMenu,
    toolbar = DefaultToolbar,
    imagesUploadUrl
}) => {
    let result = {
        selector,
        width,
        height,
        language,
        readonly: readOnly,
        setup,
        base_url: "/tinymce",
        plugins,
        menubar,
        menu,
        toolbar,
        toolbar_mode: "sliding",
        images_upload_url: imagesUploadUrl,
        resize: false
    }
    return result
}

export const HtmlEditor = ({
    active = true,
    width,
    height = 300,
    name,
    defaultValue,
    imagesUploadUrl,
    readOnly,
    shouldUnregister = false,
    addonAfter,
    brandId,
    showLocalizationAddonAfter = false,
    ...props
}) => {
    let { control } = useFormContext()
    let value = useWatch({
        name,
        control,
        defaultValue
    })

    if (!active) {
        return <Controller name={name} control={control} defaultValue={value} />
    }

    addonAfter =
        addonAfter ??
        (showLocalizationAddonAfter && (
            <LocalizationDialogDisclosure
                brandId={brandId}
                localizationField={LocalizationHtmlEditorField}
                localizationFieldProps={{
                    imagesUploadUrl
                }}
                name={name}
                readOnly={readOnly}
                mode={LocalizationDialogMode.Tab}
                dialogProps={{
                    width: "70vw",
                    maxWidth: "80vw"
                }}
            />
        ))

    return (
        <Set
            alignY="top"
            isFilled
            orientation="horizontal"
            width="100%"
            height="100%"
            flex="1"
            css={css`
                & .tox-tinymce {
                    flex: 1;
                    height: 100%;
                }

                & .tox.tox-tinymce.tox-fullscreen {
                    margin: 0;
                }
            `}
        >
            <Body
                name={name}
                readOnly={readOnly}
                shouldUnregister={shouldUnregister}
                width={width}
                height={height}
                imagesUploadUrl={imagesUploadUrl}
                {...props}
            />
            {addonAfter}
        </Set>
    )
}

const LocalizationHtmlEditorField = ({ code, readOnly, ...props }) => {
    return (
        <HtmlEditor
            name={code}
            readOnly={readOnly}
            width="100%"
            height="480"
            {...props}
        />
    )
}

const Body = ({
    name,
    defaultValue,
    imagesUploadUrl,
    required,
    readOnly,
    shouldUnregister = false,
    width,
    height = 300,
    options = {},
    onChange,
    ...props
}) => {
    let { control, setValue } = useFormContext()
    let locale = useLocale()
    let language = locale === Locales.zh ? "zh_CN" : ""

    let [editor, setEditor] = useState()

    let textareaRef = useRef()
    return (
        <Controller
            control={control}
            name={name}
            defaultValue={defaultValue}
            rules={{ required }}
            shouldUnregister={shouldUnregister}
            render={({
                field: { value, name, ...fieldProps },
                fieldState: { invalid, isTouched, isDirty, ...fieldState },
                formState
            }) => {
                const setupEditor = () => {
                    if (!textareaRef.current) {
                        return
                    }

                    let selector = `textarea[name='${name}'].${textareaRef.current.className}`
                    tinymce.remove(selector)

                    tinymce
                        .init(
                            toOptions({
                                imagesUploadUrl,
                                selector,
                                width,
                                height,
                                language,
                                readOnly,
                                setup: (editor) => {
                                    editor.on("Change", (event) => {
                                        let content = event.target.getContent()
                                        setValue?.(name, content)
                                        onChange?.(content)
                                    })
                                },
                                ...options
                            })
                        )
                        .then(([instance]) => {
                            setEditor(instance)
                        })
                }

                useEffect(() => {
                    if (textareaRef.current) {
                        setupEditor()
                    }

                    return () => {
                        if (textareaRef.current) {
                            let selector = `textarea[name='${name}'].${textareaRef.current.className}`
                            tinymce.remove(selector)
                        }
                    }
                }, [textareaRef])

                useEffect(() => {
                    if (formState.isSubmitting && invalid) {
                        editor?.focus()
                    }
                }, [formState.isSubmitting])

                return (
                    <textarea
                        name={name}
                        ref={textareaRef}
                        css={css`
                            display: none;
                        `}
                        defaultValue={value}
                        {...props}
                    />
                )
            }}
        />
    )
}
