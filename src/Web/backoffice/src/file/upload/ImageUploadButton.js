import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import {
    Box,
    Button,
    Set,
    Text,
    Flex,
    Dialog,
    Stack,
    Link,
    Group
} from "bumbag"
import { useDropzone } from "react-dropzone"
import { Trans } from "../../i18n"
import { useUploadImage } from "./client"
import { LocalizationDialogDisclosure } from "../../dialog/LocalizationDialog"
import AlertDialog from "../../dialog/Alert"
import { StaticButton } from "../../button/StaticButton"
import { useFormContext, useWatch } from "react-hook-form"

const DefaultAccept = "image/jpeg, image/png"
const DefaultMaxFiles = 1

const OneMBInBytes = 1048576
const DefaultMaxSize = OneMBInBytes * 10 //10MB

export const ImageUploadButton = ({
    name = "",
    brandId = "",
    disabled = false,
    readOnly = false,
    maxSize = DefaultMaxSize,
    onUploaded,
    ...props
}) => {
    return (
        <Set {...props}>
            <Group>
                <UploadButton name={name} disabled={disabled} />
                <LocalizationDialogDisclosure
                    brandId={brandId}
                    name={name}
                    localizationField={LocalizationUploadImageField}
                    disabled={disabled}
                    readOnly={readOnly}
                />
            </Group>
            <OpenLinkButton name={name} />
        </Set>
    )
}

const LocalizationUploadImageField = ({ code, disabled }) => {
    let name = code
    return (
        <Set>
            <Group>
                <StaticButton>{code}</StaticButton>
                <UploadButton name={name} disabled={disabled} />
            </Group>
            <OpenLinkButton name={name} />
        </Set>
    )
}

const OpenLinkButton = ({ name }) => {
    let { control } = useFormContext()
    let url = useWatch({
        control,
        name
    })
    if (!url) {
        return null
    }

    return (
        <StaticButton>
            <Link href={url} target="_blank">
                <Trans zh="查看图片" en="View Image" />
            </Link>
        </StaticButton>
    )
}

const UploadButton = ({ name, disabled, onUploaded, ...props }) => {
    let [formData, setSubmit] = useState()

    let {
        acceptedFiles = [],
        getRootProps,
        getInputProps
    } = useDropzone({
        accept: DefaultAccept,
        maxFiles: DefaultMaxFiles,
        disabled,
        maxSize: DefaultMaxSize,
        onDrop: (files = []) => {
            let [file] = files
            if (file) {
                let formData = new FormData()
                formData.append("file", file, file.name)
                setSubmit(formData)
            }
        }
    })

    let { isLoading, result, error, mutate, deleteCache } =
        useUploadImage(formData)
    disabled = disabled || isLoading

    let formMethods = useFormContext()

    useEffect(() => {
        let value = result?.location

        if (result?.valid && value) {
            onUploaded?.(value)

            if (name && formMethods?.setValue) {
                formMethods?.setValue(name, value)
            }
        }
    }, [result])

    return (
        <>
            <Button
                {...getRootProps({
                    disabled,
                    refKey: "elementRef"
                })}
            >
                <Trans zh="图片上传" en="Image Upload" />
                <input {...getInputProps()} />
                {error && <AlertDialog {...error} onClickClose={deleteCache} />}
            </Button>
        </>
    )
}

export default ImageUploadButton
