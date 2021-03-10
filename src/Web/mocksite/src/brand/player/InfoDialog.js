import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import {
    Table,
    Set,
    Button,
    Modal,
    Dialog,
    Card,
    Box,
    Input,
    Group,
    Icon,
    Stack,
    FieldStack,
    InputField,
    FieldWrapper,
    Text
} from "bumbag"
import {
    useLogout,
    usePlayerInfo,
    usePlayerToken,
    useUpdatePlayerInfo
} from "../../client/brand/player"
import { useForm } from "react-hook-form"
import { toValue } from "../../platform"

export const ViewPlayerInfoDialog = ({
    brandCode,
    currency,
    username,
    platform,
    dialog
}) => {
    let platformtype = toValue(platform)
    let { data = {} } = usePlayerInfo({
        brandCode,
        currency,
        username,
        platformtype
    })
    return (
        <Dialog.Modal title={username} showCloseButton {...dialog}>
            <Stack>
                <FieldStack orientation="horizontal">
                    <FieldWrapper label="Username">
                        <Text.Block>{data?.username}</Text.Block>
                    </FieldWrapper>
                    <FieldWrapper label="Realname">
                        <Text.Block>{data?.realname}</Text.Block>
                    </FieldWrapper>
                </FieldStack>

                <FieldWrapper label="Phone">
                    <Text.Block>{data?.phone}</Text.Block>
                </FieldWrapper>
                <FieldWrapper label="Email">
                    <Text.Block>{data?.email}</Text.Block>
                </FieldWrapper>
                <FieldWrapper label="qq">
                    <Text.Block>{data?.qq}</Text.Block>
                </FieldWrapper>
                <FieldWrapper label="wechat">
                    <Text.Block>{data?.wechat}</Text.Block>
                </FieldWrapper>
            </Stack>
        </Dialog.Modal>
    )
}

export const EditPlayerInfoDialog = ({
    brandCode,
    currency,
    username,
    platform,
    dialog
}) => {
    let errorDialog = Modal.useState()
    let platformtype = toValue(platform)
    let { data = {} } = usePlayerInfo({
        brandCode,
        currency,
        username,
        platformtype
    })
    let { register, handleSubmit, watch, setError, errors } = useForm()
    let [formData, setSubmit] = useState()
    let { result, isLoading, error, ...rest } = useUpdatePlayerInfo(
        { brandCode, currency, username, platformtype },
        formData
    )

    const onSubmit = handleSubmit(setSubmit)

    const reset = () => {
        setSubmit(undefined)
    }

    useEffect(() => {
        if (result) {
            reset()
            dialog.hide()
        }
    }, [result])

    useEffect(() => {
        if (error) {
            errorDialog.show()
        }
    }, [error])

    return (
        <>
            <Dialog.Modal
                title={username}
                showCloseButton
                showActionButtons
                hideOnClickOutside={false}
                actionButtonsProps={{
                    isLoading,
                    onClickSubmit: onSubmit
                }}
                {...dialog}
            >
                <form onSubmit={onSubmit}>
                    <FieldStack>
                        <FieldStack orientation="horizontal">
                            <InputField
                                name="username"
                                defaultValue={data?.username}
                                label="Username"
                                disabled
                            />
                            <InputField
                                name="realname"
                                inputRef={register}
                                defaultValue={data?.realname}
                                label="Realname"
                            />
                        </FieldStack>
                        <InputField
                            name="phone"
                            inputRef={register}
                            defaultValue={data?.phone}
                            label="Phone"
                        />
                        <InputField
                            name="email"
                            type="email"
                            inputRef={register}
                            defaultValue={data?.email}
                            label="Email"
                        />
                        <InputField
                            name="qq"
                            inputRef={register}
                            defaultValue={data?.qq}
                            label="QQ"
                        />
                        <InputField
                            name="wechat"
                            inputRef={register}
                            defaultValue={data?.wechat}
                            label="Wechat"
                        />
                    </FieldStack>
                </form>
            </Dialog.Modal>
            {error && (
                <Modal.Backdrop zIndex="19900410" {...errorDialog}>
                    <Dialog.Modal
                        title="Error"
                        showCloseButton
                        type="warning"
                        hideBackdrop={true}
                        onClickClose={reset}
                        {...errorDialog}
                    >
                        <Text.Block>{JSON.stringify(error)}</Text.Block>
                    </Dialog.Modal>
                </Modal.Backdrop>
            )}
        </>
    )
}
