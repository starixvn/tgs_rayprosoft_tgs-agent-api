import React, { useState, useEffect, useMemo, useRef, useContext } from "react"
import { css, Global } from "@emotion/react"
import { useEditPlayerInfo } from "./client"
import {
    Spinner,
    Stack,
    Flex,
    Box,
    Card,
    Set,
    Button,
    Text,
    Dialog,
    Modal,
    Table,
    useClassName
} from "bumbag"
import { FieldValues } from "../../layout/Grid"
import {
    Trans,
    FormattedDateTime,
    FormattedNumber,
    useTrans,
    FormattedNumberDecimal
} from "../../i18n"
import { PlayerStatusSelect, PlayerStatusLabel } from "../PlayerStatus"
import { VipLevelSelect } from "../../vip-levels/VipLevelSelect"
import AlertDialog from "../../dialog/Alert"
import { SubmitCancel } from "../../button/ActionButtons"
import { Tab as _Tab, TabPanel } from "../../tab"
import { FormProvider, useForm, useFormContext } from "react-hook-form"
import Input from "../../input/Input"
import clone from "clone-deep"
import { VipLevelLabel } from "../../vip-levels/VipLevel"
import { PlayerClearWageringRequirementDisclosure } from "./PlayerClearWageringRequirement"
import { useIsAdminUser } from "../../user/context"

export const Key = "player"

export const Tab = ({ ...props }) => {
    return (
        <_Tab tabId={Key} {...props}>
            <Trans zh="玩家信息" en="Player" />
        </_Tab>
    )
}

export const EditPanel = ({
    brandId,
    playerCode = "",
    player = {},
    onSubmitted,
    onClickClose,
    ...props
}) => {
    let formMethods = useForm({
        defaultValues: clone(player)
    })

    let [submitData, setSubmit] = useState()
    let { isLoading, result, error, mutate } = useEditPlayerInfo(
        playerCode,
        submitData
    )

    const reset = () => {
        mutate(undefined, false)
        setSubmit(undefined)
    }

    useEffect(() => {
        if (result && !error) {
            reset()
            onSubmitted?.()
        }
    }, [result])

    const onSubmit = (data = {}) => {
        setSubmit(data)
    }

    return (
        <TabPanel tabId={Key} {...props}>
            {error && <AlertDialog {...error} onClickClose={reset} />}
            <FormProvider {...formMethods}>
                <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                    <EditForm
                        brandId={brandId}
                        playerCode={playerCode}
                        player={player}
                        isLoading={isLoading}
                        onClickClose={() => {
                            reset()
                            onClickClose?.()
                        }}
                    />
                </form>
            </FormProvider>
        </TabPanel>
    )
}

const EditForm = ({
    brandId,
    playerCode = "",
    player = {},
    isLoading,
    onClickClose,
    ...props
}) => {
    let { setValue } = useFormContext()
    let isAdmin = useIsAdminUser()
    let agentUsernamePlaceholder = useTrans({
        zh: "代理用户名",
        en: "Agent Username"
    })
    let { username = "", registeredOn, balance } = player

    return (
        <Stack spacing="major-1" margin="major-1">
            <FieldValues>
                {isAdmin && (
                    <>
                        <Trans zh="Player Code" en="Player Code" />
                        <Text>{player.playerCode}</Text>
                    </>
                )}
                <UserNameLabel />
                <Text>{username}</Text>

                <RealNameLabel />
                <Box>
                    <Input name="realName" />
                </Box>

                <VIPLevelLabel />
                <Box>
                    <VipLevelSelect
                        name="vipId"
                        defaultValue={player?.vipId}
                        required
                        brandId={brandId}
                    />
                </Box>

                <AgentUsernameLabel />
                <Box>
                    <Input
                        name="agentUsername"
                        placeholder={agentUsernamePlaceholder}
                    />
                </Box>

                <RegistrationLabel />
                <Box>
                    <FormattedDateTime value={registeredOn} />
                </Box>

                <BalanceLabel />
                <Box>
                    <FormattedNumberDecimal value={balance} />
                </Box>

                <Box alignY="center">
                    <WageringRequirementLabel />
                </Box>
                <Set>
                    <FormattedNumberDecimal
                        value={player?.wageringRequirement}
                    />
                    {/* <PlayerClearWageringRequirementDisclosure
                        playerCode={player?.playerCode}
                        onSubmitted={() => {
                            player.wageringRequirement = 0
                        }}
                    /> */}
                </Set>

                <StatusLabel />
                <Box>
                    <PlayerStatusSelect
                        name="status"
                        required
                        placeholder={false}
                    />
                </Box>
            </FieldValues>
            <Box alignX="right">
                <SubmitCancel
                    onClickCancel={onClickClose}
                    isLoading={isLoading}
                />
            </Box>
        </Stack>
    )
}

export const Panel = ({ player = {}, onSubmitted, ...props }) => {
    let isAdmin = useIsAdminUser()
    let {
        username = "",
        realName = "",
        vipLevel = "",
        registeredOn,
        balance,
        status
    } = player

    return (
        <TabPanel tabId={Key} {...props}>
            <FieldValues spacing="major-2">
                {isAdmin && (
                    <>
                        <Trans zh="Player Code" en="Player Code" />
                        <Text>{player.playerCode}</Text>
                    </>
                )}
                <UserNameLabel />
                <Text>{username}</Text>

                <RealNameLabel />
                <Box>
                    <Text>{realName}</Text>
                </Box>

                <VIPLevelLabel />
                <Box>
                    <VipLevelLabel value={vipLevel} />
                </Box>

                <AgentUsernameLabel />
                <Box>
                    <Text>{player?.agentUsername}</Text>
                </Box>

                <RegistrationLabel />
                <Box>
                    <FormattedDateTime value={registeredOn} />
                </Box>

                <BalanceLabel />
                <Box>
                    <FormattedNumberDecimal value={balance} />
                </Box>

                <Box alignY="center">
                    <WageringRequirementLabel />
                </Box>
                <Box>
                    <FormattedNumberDecimal
                        value={player?.wageringRequirement}
                    />
                </Box>

                <StatusLabel />
                <Box>
                    <PlayerStatusLabel value={status} />
                </Box>
            </FieldValues>
        </TabPanel>
    )
}

const UserNameLabel = () => <Trans zh="用户名" en="User name" />

const RealNameLabel = () => <Trans zh="真实姓名" en="Real name" />

const VIPLevelLabel = () => <Trans zh="VIP等级" en="VIP Level" />

const AgentUsernameLabel = () => <Trans zh="上级代理" en="Upper Level Agent" />

const RegistrationLabel = () => <Trans zh="注册时间" en="Registration" />

const BalanceLabel = () => <Trans zh="余额" en="Balance" />

const WageringRequirementLabel = () => (
    <Trans zh="需滿足提款金額" en="Wagering requirement" />
)

const StatusLabel = () => <Trans zh="状态" en="Status" />
