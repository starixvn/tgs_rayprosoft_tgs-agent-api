import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import { Box, Set, Link, Group, Button, Icon } from "bumbag"
import { Trans } from "../../i18n"
import { useUploadImage } from "./client"
import { LocalizationDialogDisclosure } from "../../dialog/LocalizationDialog"
import { StaticButton } from "../../button/StaticButton"
import { useFormContext, useWatch } from "react-hook-form"
import FileUploadButton from "./FileUploadButton"

const DefaultAccept = "image/jpeg, image/png"

export const ImageUploadButton = ({
    name = "",
    brandId = "",
    prefix = "",
    disabled = false,
    readOnly = false,
    onUploaded,
    ...props
}) => {
    return (
        <Set {...props}>
            <Group>
                <UploadButton name={name} prefix={prefix} disabled={disabled} />
                <LocalizationDialogDisclosure
                    brandId={brandId}
                    name={name}
                    localizationField={LocalizationUploadImageField}
                    localizationFieldProps={{
                        prefix
                    }}
                    disabled={disabled}
                    readOnly={readOnly}
                />
            </Group>
            <OpenLinkButton name={name} disabled={disabled} />
        </Set>
    )
}

const LocalizationUploadImageField = ({ code, prefix, disabled }) => {
    let name = code
    return (
        <Set>
            <Group>
                <StaticButton>{code}</StaticButton>
                <UploadButton name={name} prefix={prefix} disabled={disabled} />
            </Group>
            <OpenLinkButton name={name} disabled={disabled} />
        </Set>
    )
}

const OpenLinkButton = ({ name, disabled }) => {
    let { control, setValue } = useFormContext()
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
            {!disabled && (
                <Button.Close
                    onClick={(event) => {
                        setValue(name, "")
                    }}
                    disabled={disabled}
                />
            )}
        </StaticButton>
    )
}

const UploadButton = ({ name, prefix, disabled, onUploaded, ...props }) => {
    const useUpload = (body) => useUploadImage(prefix, body)
    return (
        <FileUploadButton
            name={name}
            disabled={disabled}
            accept={DefaultAccept}
            useUpload={useUpload}
            onUploaded={onUploaded}
        >
            <Trans zh="图片上传" en="Image Upload" />
        </FileUploadButton>
    )
}

export default ImageUploadButton
