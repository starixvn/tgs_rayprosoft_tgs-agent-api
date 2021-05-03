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
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserBrandId
} from "../../user/context"
import { Code, Update } from "./menu"
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
import { PlayerAddBalanceDisclosure } from "./PlayerAddBalance"
import { PlayerDeductBalanceDisclosure } from "./PlayerDeductBalance"
import { usePlayerList } from "../player-list/client"

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
        username: ""
    })

    useEffect(() => {
        setState({ pageIndex: 0, brandId })
    }, [brandId])

    let { pageIndex, pageSize, username } = state
    let { data = {}, isLoading, mutate, error, deleteCache } = usePlayerList(
        brandId
            ? {
                  ...toApiParams(state),
                  sortby: "username",
                  brandId
              }
            : undefined
    )

    let columns = useColumns({ mutate, items: data?.items, brandId })

    return (
        <PageContent
            title={<Trans zh="玩家列表" en="Players" />}
            actionBar={
                <Flex justifyContent="space-between">
                    <SearchBar
                        brandId={brandId}
                        search={(data) => {
                            let newState = {
                                pageIndex: 0,
                                username: data.username
                            }
                            if (
                                equals(newState, {
                                    pageIndex,
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
        zh: "用户名",
        en: "Username"
    })
    let formMethods = useForm()

    const onSubmit = (data) => search(data)

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                <Set>
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
                Header: <Trans zh="用户名" en="Username" />,
                disableSortBy: true
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
                                <PlayerAddBalanceDisclosure
                                    brandId={brandId}
                                    username={username}
                                    onSubmitted={mutate}
                                />
                                <PlayerDeductBalanceDisclosure
                                    brandId={brandId}
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
