import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import {
    Heading,
    Stack,
    Box,
    Button,
    Set,
    useModalState,
    Flex,
    Level
} from "bumbag"
import PageContent from "../../page/Content"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { AgentStatusLabel } from "../AgentStatus"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserBrandId
} from "../../user/context"
import { Code, Update } from "./menu"
import { AgentStatusSelect } from "../AgentStatus"
import { VipLevelSelect } from "../../vip-levels/VipLevelSelect"
import { FormattedNumberDecimal, Trans, useTrans } from "../../i18n"
import SearchButton from "../../button/SearchButton"
import equals from "fast-deep-equal"
import { InputField } from "../../input/Input"
import BrandBar from "../../report/BrandBar"
import AlertDialog from "../../dialog/Alert"
import textColumn from "../../table/column/textColumn"
import numberColumn, {
    numberDecimalColumn,
    numberDecimalPercentageColumn
} from "../../table/column/numberColumn"
import { AgentLevelLabel, AgentLevelSelect } from "../AgentLevel"
import { AgentTypeLabel, AgentTypeSelect } from "../AgentType"
import { useAgentList } from "../agent-list/client"
import { AgentAddBalanceDisclosure } from "./AgentAddBalance"
import { AgentDeductBalanceDisclosure } from "./AgentDeductBalance"

export { Code }

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

export const CodeContentPair = [Code, Content]

const Body = () => {
    let userBrandId = useUserBrandId()
    let [brandId, setBrandId] = useState(userBrandId)

    return (
        <Set orientation="vertical" isFilled>
            <BrandBar
                onBrandChange={(value) => {
                    setBrandId(value)
                }}
            />
            {brandId && <BodyContent brandId={brandId} />}
        </Set>
    )
}

const BodyContent = ({ brandId = "" }) => {
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        status: "",
        levelType: "",
        type: "",
        username: ""
    })

    useEffect(() => {
        setState({ pageIndex: 0, brandId })
    }, [brandId])

    let { pageIndex, pageSize, status, levelType, type, username } = state
    let { data = {}, isLoading, mutate, error, deleteCache } = useAgentList({
        ...toApiParams(state),
        sortby: "username",
        brandId
    })

    let columns = useColumns({ mutate, items: data?.items, brandId })

    return (
        <PageContent
            title={<Trans zh="代理调账" en="Agent Balance Adjustment" />}
            actionBar={
                <Flex justifyContent="space-between">
                    <SearchBar
                        brandId={brandId}
                        search={(data) => {
                            let newState = {
                                pageIndex: 0,
                                status: data.status,
                                levelType: data.levelType,
                                type: data.type,
                                username: data.username
                            }
                            if (
                                equals(newState, {
                                    pageIndex,
                                    status,
                                    levelType,
                                    type,
                                    username
                                })
                            ) {
                                mutate()
                            }
                            setState({
                                pageIndex: 0,
                                ...data
                            })
                        }}
                        isLoading={isLoading}
                    />
                    <Box></Box>
                </Flex>
            }
        >
            <Table
                controlledState={state}
                columns={columns}
                data={data?.items}
                pageCount={pageCount(pageSize, data?.totalCount)}
                setPageIndex={setPageIndex}
                setSortOrder={setSortOrder}
                isLoading={isLoading}
            />
            {error && (
                <AlertDialog
                    {...error}
                    onClickClose={() => {
                        deleteCache()
                        mutate({ data: undefined }, false)
                    }}
                />
            )}
        </PageContent>
    )
}

const SearchBar = ({ brandId, search, isLoading, ...props }) => {
    let usernameInputPlaceholder = useTrans({
        zh: "用户名输入框",
        en: "Input username"
    })
    let formMethods = useForm()

    const onSubmit = (data) => search(data)

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                <Set>
                    <AgentStatusSelect name="status" />
                    <AgentLevelSelect name="levelType" />
                    <AgentTypeSelect name="type" />
                    <InputField
                        name="username"
                        showEmptyFieldValueAddonAfter
                        placeholder={usernameInputPlaceholder}
                    />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}
const useColumns = ({ brandId, items = [], mutate }) => {
    let hasUpdateMenu = useIsDisplayMenuByRoute([Update])

    return useMemo(() => {
        let columns = [
            sequenceColumn(),
            textColumn({
                accessor: "username",
                Header: <Trans zh="代里用户名" en="Agent Username" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "levelType",
                Header: <Trans zh="代理种类" en="Level Type" />,
                disableSortBy: true,
                Cell: ({ value }) => <AgentLevelLabel value={value} />
            }),
            textColumn({
                accessor: "type",
                Header: <Trans zh="代理类型" en="Agent Type" />,
                disableSortBy: true,
                Cell: ({ value }) => <AgentTypeLabel value={value} />
            }),
            numberDecimalColumn({
                accessor: "balance",
                Header: <Trans zh="余额" en="Balance" />,
                disableSortBy: true
            })
        ]

        if (hasUpdateMenu) {
            columns.push(
                textColumn({
                    id: "actions",
                    accessor: (row) => row,
                    disableSortBy: true,
                    Header: <Trans zh="操作" en="Actions" />,
                    Cell: ({ value }) => {
                        let username = value?.username
                        return (
                            <Set alignX="center">
                                <AgentAddBalanceDisclosure
                                    brandId={brandId}
                                    agentId={value?.id}
                                    username={username}
                                    onSubmitted={mutate}
                                />
                                <AgentDeductBalanceDisclosure
                                    brandId={brandId}
                                    agentId={value?.id}
                                    username={username}
                                    onSubmitted={mutate}
                                />
                            </Set>
                        )
                    }
                })
            )
        }

        return columns
    }, [JSON.stringify(items)])
}
