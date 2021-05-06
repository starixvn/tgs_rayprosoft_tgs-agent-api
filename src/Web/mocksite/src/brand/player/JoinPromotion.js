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
    Select
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
    useInitPlayerPayPassword,
    useJoinPromotion,
    usePromotionList
} from "../../client/brand/player"
import { useForm } from "react-hook-form"
import { toValue } from "../../platform"

export const JoinPromotionDialog = ({
    brandCode,
    currency,
    username,
    platform,
    dialog
}) => {
    let { register, handleSubmit, watch, setError, errors } = useForm()
    let [promotionId, setSubmit] = useState()
    let platformtype = toValue(platform)
    let { result, error, isLoading, mutate, ...rest } = useJoinPromotion(
        { brandCode, currency, username, platformtype },
        promotionId
    )
    const onSubmit = handleSubmit(({ id }) => {
        setSubmit(id)
    })

    const reset = () => {
        setSubmit(undefined)
        mutate(undefined, false)
    }

    useEffect(() => {
        if (result) {
            reset()
            dialog.hide()
        }
    }, [result])

    const errorDialog = Modal.useState()
    useEffect(() => {
        if (error) {
            errorDialog.show()
        }
    }, [error])

    return (
        <>
            <Dialog.Modal
                title="Join Promotion"
                showCloseButton
                showActionButtons
                hideOnClickOutside={false}
                actionButtonsProps={{
                    isLoading,
                    onClickSubmit: onSubmit
                }}
                {...dialog}
            >
                <FieldStack>
                    <PromotionSelect
                        name="id"
                        selectRef={register}
                        placeholder="Select a Promotion"
                        required
                        brandCode={brandCode}
                        currency={currency}
                        username={username}
                        platform={platform}
                    />
                </FieldStack>
            </Dialog.Modal>
            {error && (
                <Modal.Backdrop zIndex="19900410" {...errorDialog}>
                    <Dialog.Modal
                        title="Error"
                        showCloseButton
                        type="warning"
                        hideBackdrop={true}
                        onClickClose={() => {
                            reset()
                        }}
                        {...errorDialog}
                    >
                        <Text.Block>{JSON.stringify(error)}</Text.Block>
                    </Dialog.Modal>
                </Modal.Backdrop>
            )}
        </>
    )
}

const PromotionSelect = ({
    brandCode,
    currency,
    username,
    platform,
    ...props
}) => {
    let platformtype = toValue(platform)
    let { isLoading, data = [], error } = usePromotionList({
        brandCode,
        currency,
        username,
        platformtype
    })

    let options = data?.map?.(
        (item) =>
            ({
                label: item.name,
                value: item.id
            } ?? [])
    )

    return <Select isLoading={isLoading} options={options} {...props} />
}
