import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import {
    Table,
    Set,
    Button,
    Stack,
    FieldStack,
    Box,
    PageWithHeader,
    TopNav,
    DropdownMenu,
    Group,
    Card,
    Dialog,
    Modal,
    Text,
    FieldWrapper,
    InputField,
    Select,
    SelectField,
    Heading
} from "bumbag"
import {
    Switch,
    Route,
    Link,
    useParams,
    useRouteMatch,
    useLocation,
    useHistory
} from "react-router-dom"
import BrandHeader from "../Header"
import PlatformDropdownMenu from "../../platform/DropdownMenu"
import LangDropdownMenu from "../../language/DropdownMenu"
import { GameProviderDropdownMenu } from "../../game/provider/DropdownMenu"
import { GameList } from "../../game/List"
import {
    useApplyDeposit,
    usePaymentList,
    useUploadDepositProof
} from "../../client/brand/player"
import { useForm } from "react-hook-form"
import { toValue } from "../../platform"
import { useDropzone } from "react-dropzone"

export const UploadDepositProofDialog = ({
    brandCode,
    currency,
    username,
    platform,
    depositFormId,
    dialog
}) => {
    let platformtype = toValue(platform)
    let [formData, setSubmit] = useState()

    const {
        acceptedFiles,
        getRootProps,
        getInputProps,
        isDragActive,
        isDragAccept,
        isDragReject
    } = useDropzone({
        maxFiles: 1,
        accept: "image/jpeg, image/png",
        onDrop: (files) => {
            if (files.length > 0) {
                let [file] = files
                let formData = new FormData()
                formData.append("file", file, file.name)
                setSubmit(formData)
            }
        }
    })

    let { isLoading, result, error, mutate } = useUploadDepositProof(
        { brandCode, currency, username, platformtype, depositFormId },
        formData
    )

    useEffect(() => {
        if (result) {
            // reset()
            dialog.hide()
        }
    }, [result])

    return (
        <>
            <Dialog.Modal
                title="Upload Deposit Proof"
                showCloseButton
                hideOnClickOutside={false}
                {...dialog}
            >
                depositFormId: {depositFormId}
                <Card
                    {...getRootProps({
                        refKey: "elementRef "
                    })}
                    css={css`
                        flex: 1;
                        display: flex;
                        flex-direction: column;
                        align-items: center;
                        padding: 20px;
                        border-width: 2px;
                        border-radius: 2px;
                        border-color: ${isDragActive || isDragAccept
                            ? "#2196f3"
                            : "#eeeeee"};
                        border-style: dashed;
                        background-color: #fafafa;
                        color: #bdbdbd;
                        outline: none;
                        transition: border 0.24s ease-in-out;
                    `}
                >
                    <input {...getInputProps()} />
                    <p>Upload File</p>
                </Card>
            </Dialog.Modal>
        </>
    )
}
