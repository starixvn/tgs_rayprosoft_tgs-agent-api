import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext, Controller } from "react-hook-form"
import { Locales, useLocale } from "../../i18n"

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

const toOptions = ({
    selector,
    height,
    readOnly,
    setup,
    language,
    plugins = DefaultPlugin,
    menubar = DefaultMenubar,
    menu = DefaultMenu,
    toolbar = DefaultToolbar
}) => ({
    selector,
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
    images_upload_url: "/fs/file/uploadFileS3",
    resize: false
})

//TODO: has disabled
export const HtmlEditor = ({
    name = "",
    defaultValue,
    required,
    readOnly,
    /*
    disabled,
    */
    height = 300,
    options = {},
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
            shouldUnregister={true}
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
                    console.log("setupEditor by selector", selector)
                    tinymce.remove(selector)

                    tinymce
                        .init(
                            toOptions({
                                selector,
                                height,
                                language,
                                readOnly,
                                setup: (editor) => {
                                    editor.on("Change", (event) => {
                                        let content = event.target.getContent()
                                        setValue(name, content)
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
                            console.log(
                                "Cleaning useEffect by selector",
                                selector
                            )
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
