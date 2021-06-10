import React, { useState, useEffect, useMemo } from "react"
import { css, Stack, Heading, Box, Button, Text } from "bumbag"
import { useModalState } from "../../dialog/Modal"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { Update as UpdateMenu } from "./menu"
import { EditDisclosure } from "../../dialog/Disclosure"
import { ConfirmButtonFooter } from "../../form/ConfirmButtonFooter"
import { useUpdateBrandGame } from "./client"
import { Trans, useTrans } from "../../i18n"
import { FieldValues } from "../../layout/Grid"
import Label from "../../form/Label"
import FormDialog from "../../form/FormDialog"
import { GameTypeLabel, isLobbyGameType, notLobbyGameType } from "../GameType"
import { GamePlatformLabel } from "../games/GamePlatform"
import { useFormContext } from "react-hook-form"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import { useBrandGame } from "./client"
import AlertDialog from "../../dialog/Alert"
import Switch from "../../switch/Switch"
import { PercentageNumberInputField } from "../../input/NumberInput"

export const EditButton = ({ record, onSubmitted, ...props }) => {
    let dialog = useModalState()
    let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <EditDisclosure {...dialog} />
            {dialog.visible && (
                <EditDialog
                    record={record}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const EditDialog = ({ record = {}, dialog, ...props }) => {
    let { id } = record
    let {
        data: brandGame,
        isLoading: isLoadingBrandGame,
        error: brandGameError,
        mutate: mutatebrandGame,
        deleteCache: deleteBrandGameCache
    } = useBrandGame(id)

    let [submitData, setSubmit] = useState()
    let { result, isLoading, error, mutate, deleteCache } = useUpdateBrandGame(
        record?.id,
        submitData
    )

    const onSubmit = (formData) => {
        setSubmit(formData)
    }

    const reset = () => {
        mutatebrandGame(undefined, false)
        deleteBrandGameCache()

        mutate(undefined, false)
        setSubmit(undefined)
    }

    let title = useTrans({
        zh: "编辑",
        en: "Edit"
    })

    if (isLoadingBrandGame) {
        return <LoadingBackdrop {...dialog} />
    }

    if (brandGameError) {
        return <AlertDialog {...brandGameError} onClickClose={dialog.hide} />
    }

    return (
        <FormDialog
            defaultValues={{ ...brandGame }}
            onSubmit={onSubmit}
            result={result}
            isLoading={isLoading}
            error={error}
            onCloseErrorAlert={reset}
            onSubmitted={() => {
                reset()
                props?.onSubmitted()
            }}
            title={title}
            showCloseButton
            showActionButtons={false}
            dialog={dialog}
            footer={
                <ConfirmButtonFooter
                    isLoading={isLoading}
                    disabled={isLoading}
                />
            }
            maxWidth="unset"
        >
            <EditBody record={brandGame} />
        </FormDialog>
    )
}

const EditBody = ({ record = {} }) => {
    let {
        gameProviderName,
        gameType,
        gameName,
        gameId,
        platform,
        supportHtml5
    } = record

    let notLobbyGame = notLobbyGameType(gameType)
    return (
        <Stack orientation="vertical" minWidth="40vw" marginLeft="1rem">
            <Heading use="h5" marginBottom="major-1">
                <Trans zh="基本资料" en="Basic Information" />
            </Heading>
            <Stack>
                <Stack orientation="horizontal" marginBottom="major-1">
                    <FieldValues>
                        <Label fontWeight="normal">
                            <Trans zh="厂商" en="Vendor" />
                        </Label>
                        <Box alignY="center">{gameProviderName}</Box>
                    </FieldValues>

                    <FieldValues>
                        <Label fontWeight="normal">
                            <Trans zh="游戏类型" en="Game Type" />
                        </Label>
                        <Box alignY="center">
                            <GameTypeLabel value={gameType} />
                        </Box>
                    </FieldValues>
                </Stack>
                <Stack orientation="horizontal" marginBottom="major-1">
                    <FieldValues>
                        <Label fontWeight="normal">
                            <Trans zh="游戏名称" en="Game Name" />
                        </Label>
                        <Box alignY="center">{gameName}</Box>
                    </FieldValues>
                    <FieldValues>
                        <Label fontWeight="normal">
                            <Trans zh="游戏 ID" en="Game ID" />
                        </Label>
                        <Box alignY="center">{gameId}</Box>
                    </FieldValues>
                </Stack>
                <Stack orientation="horizontal" marginBottom="major-1">
                    <FieldValues>
                        <Label fontWeight="normal">
                            <Trans zh="支持设备" en="Platform" />
                        </Label>
                        <Box alignY="center">
                            <GamePlatformLabel value={platform} />
                        </Box>
                    </FieldValues>

                    <FieldValues>
                        <Label fontWeight="normal">
                            <Trans zh="支持HTML5" en="HTML5" />
                        </Label>
                        <Box alignY="center">
                            {supportHtml5 ? (
                                <Trans zh="是" en="Yes" />
                            ) : (
                                <Trans zh="不" en="No" />
                            )}
                        </Box>
                    </FieldValues>
                </Stack>
            </Stack>

            {notLobbyGame && (
                <>
                    <Heading use="h5" marginBottom="major-1">
                        <Trans zh="特殊处理" en="Special Processing" />
                    </Heading>
                    <FieldValues>
                        <Label fontWeight="normal">
                            <Trans
                                zh="有效投注比例"
                                en="Effective Betting Proportion"
                            />
                        </Label>
                        <Box alignY="center">
                            <PercentageNumberInputField name="effectiveBettingPercentage" />
                        </Box>

                        <Label fontWeight="normal">
                            <Trans zh="是否发放返水" en="Commission Rebate" />
                        </Label>
                        <Switch name="rebate" />
                    </FieldValues>
                </>
            )}
        </Stack>
    )
}
