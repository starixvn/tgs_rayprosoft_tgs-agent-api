import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import { Box, Button, Set, Text, Flex, Dialog, Stack } from "bumbag"
import { useDropzone } from "react-dropzone"
import { Trans } from "../../i18n"

const DefaultAccept = "image/jpeg, image/png"
const DefaultMaxFiles = 1

const OneMBInBytes = 1048576
const DefaultMaxSize = OneMBInBytes * 5 //5MB
//TODO
export const ImageUploadButtons = ({
    accept = DefaultAccept,
    maxFiles = DefaultMaxFiles,
    disabled = false,
    maxSize = DefaultMaxSize,
    onDrop,
    uploadButtonProps = {},
    ...props
}) => {
    let {
        acceptedFiles = [],
        getRootProps,
        getInputProps
    } = useDropzone({
        accept,
        maxFiles,
        disabled,
        maxSize,
        onDrop
    })

    return (
        <Set {...props}>
            <Button
                {...getRootProps({
                    ...uploadButtonProps,
                    refKey: "elementRef"
                })}
            >
                <Trans zh="图片上传" en="Image Upload" />
                <input {...getInputProps()} />
            </Button>
            {acceptedFiles.map((uploadedFile) => (
                <UploadedImageButton file={uploadedFile} />
            ))}
        </Set>
    )
}

const UploadedImageButton = ({ file, ...props }) => {}
